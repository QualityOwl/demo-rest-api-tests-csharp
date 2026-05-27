import http from 'k6/http';
import { BASE_URL, API_USERNAME, API_PASSWORD } from '../config/environments.js';

export function getAuthToken() {
  const payload = JSON.stringify({
    username: API_USERNAME,
    password: API_PASSWORD,
  });

  const params = {
    headers: { 'Content-Type': 'application/json' },
  };

  const res = http.post(`${BASE_URL}/auth`, payload, params);
  return res.json('token');
}
