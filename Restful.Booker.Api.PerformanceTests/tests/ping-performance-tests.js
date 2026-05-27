import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL } from '../config/environments.js';
import { defaultThresholds } from '../config/thresholds.js';

export const options = {
  stages: [
    { duration: '10s', target: 5 },
    { duration: '20s', target: 5 },
    { duration: '10s', target: 0 },
  ],
  thresholds: {
    ...defaultThresholds,
    'http_req_duration{endpoint:ping}': ['p(95)<1000'],
  },
};

export default function () {
  const res = http.get(`${BASE_URL}/ping`, {
    tags: { endpoint: 'ping' },
  });

  check(res, {
    'GET /ping returns 201 Created': (r) => r.status === 201,
  });

  sleep(1);
}
