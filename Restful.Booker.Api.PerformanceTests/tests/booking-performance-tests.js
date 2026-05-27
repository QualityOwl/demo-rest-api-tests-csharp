import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL } from '../config/environments.js';
import { defaultThresholds } from '../config/thresholds.js';
import { getAuthToken } from '../helpers/auth-helper.js';

export const options = {
  stages: [
    { duration: '10s', target: 5 },
    { duration: '30s', target: 10 },
    { duration: '10s', target: 0 },
  ],
  thresholds: {
    ...defaultThresholds,
    'http_req_duration{endpoint:get_bookings}': ['p(95)<2000'],
    'http_req_duration{endpoint:get_booking_by_id}': ['p(95)<2000'],
    'http_req_duration{endpoint:create_booking}': ['p(95)<3000'],
    'http_req_duration{endpoint:update_booking}': ['p(95)<3000'],
    'http_req_duration{endpoint:delete_booking}': ['p(95)<3000'],
  },
};

export function setup() {
  return { token: getAuthToken() };
}

export default function (data) {
  let bookingId;

  const listRes = http.get(`${BASE_URL}/booking`, {
    tags: { endpoint: 'get_bookings' },
  });

  check(listRes, {
    'GET /booking returns 200': (r) => r.status === 200,
    'GET /booking returns array': (r) => Array.isArray(r.json()),
  });

  const createPayload = JSON.stringify({
    firstname: 'K6',
    lastname: 'PerformanceTest',
    totalprice: 100,
    depositpaid: true,
    bookingdates: {
      checkin: '2025-01-01',
      checkout: '2025-01-05',
    },
    additionalneeds: 'Performance test booking',
  });

  const createRes = http.post(`${BASE_URL}/booking`, createPayload, {
    headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
    tags: { endpoint: 'create_booking' },
  });

  check(createRes, {
    'POST /booking returns 200': (r) => r.status === 200,
    'POST /booking returns booking id': (r) => r.json('bookingid') > 0,
  });

  if (createRes.status === 200) {
    bookingId = createRes.json('bookingid');
  }

  if (bookingId) {
    const getRes = http.get(`${BASE_URL}/booking/${bookingId}`, {
      headers: { Accept: 'application/json' },
      tags: { endpoint: 'get_booking_by_id' },
    });

    check(getRes, {
      'GET /booking/{id} returns 200': (r) => r.status === 200,
      'GET /booking/{id} has correct firstname': (r) => r.json('firstname') === 'K6',
    });

    const updatePayload = JSON.stringify({
      firstname: 'K6',
      lastname: 'Updated',
      totalprice: 200,
      depositpaid: false,
      bookingdates: {
        checkin: '2025-01-01',
        checkout: '2025-01-10',
      },
    });

    const putRes = http.put(`${BASE_URL}/booking/${bookingId}`, updatePayload, {
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Cookie: `token=${data.token}`,
      },
      tags: { endpoint: 'update_booking' },
    });

    check(putRes, {
      'PUT /booking/{id} returns 200': (r) => r.status === 200,
      'PUT /booking/{id} has updated lastname': (r) => r.json('lastname') === 'Updated',
    });

    const deleteRes = http.del(`${BASE_URL}/booking/${bookingId}`, null, {
      headers: { Cookie: `token=${data.token}` },
      tags: { endpoint: 'delete_booking' },
    });

    check(deleteRes, {
      'DELETE /booking/{id} returns 201': (r) => r.status === 201,
    });
  }

  sleep(1);
}
