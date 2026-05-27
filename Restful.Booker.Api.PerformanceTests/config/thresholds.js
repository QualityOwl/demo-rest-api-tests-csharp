// Threshold definitions shared across all performance test scripts.
export const defaultThresholds = {
  http_req_failed: ['rate<0.01'],
  http_req_duration: ['p(95)<3000', 'p(99)<5000'],
  checks: ['rate>0.95'],
};
