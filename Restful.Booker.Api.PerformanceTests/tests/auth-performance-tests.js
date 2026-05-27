import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL, API_USERNAME, API_PASSWORD } from '../config/environments.js';
import { defaultThresholds } from '../config/thresholds.js';

export const options = {
  stages: [
    { duration: '10s', target: 5 },
    { duration: '20s', target: 5 },
    { duration: '10s', target: 0 },
  ],
  thresholds: {
    ...defaultThresholds,
    'http_req_duration{endpoint:auth}': ['p(95)<2000'],
  },
};

export default function () {
  const payload = JSON.stringify({
    username: API_USERNAME,
    password: API_PASSWORD,
  });

  const params = {
    headers: { 'Content-Type': 'application/json' },
    tags: { endpoint: 'auth' },
  };

  const res = http.post(`${BASE_URL}/auth`, payload, params);

  check(res, {
    'POST /auth returns 200': (r) => r.status === 200,
    'POST /auth response contains token': (r) =>
      r.json('token') !== undefined && r.json('token') !== null,
  });

  sleep(1);
}
