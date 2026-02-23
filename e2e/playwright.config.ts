import { defineConfig, devices } from '@playwright/test';

/**
 * Playwright configuration for Game Manager smoke tests
 * 
 * Environment variables:
 * - BASE_URL (required): Base URL of the application to test (e.g., http://localhost:5000)
 * - TEST_TIMEOUT (optional): Test timeout in milliseconds (default: 30000)
 * - TEST_RETRIES (optional): Number of retries for flaky tests (default: 1)
 * - HEADLESS (optional): Run in headless mode (default: true)
 * - IGNORE_HTTPS_ERRORS (optional): Ignore HTTPS/certificate errors (default: false)
 */

const baseURL = process.env.BASE_URL;
if (!baseURL) {
  throw new Error('BASE_URL environment variable is required. Example: BASE_URL=http://localhost:5000');
}

const testTimeout = parseInt(process.env.TEST_TIMEOUT || '30000', 10);
const retries = parseInt(process.env.TEST_RETRIES || '1', 10);
const headless = process.env.HEADLESS !== 'false';
const ignoreHTTPSErrors = process.env.IGNORE_HTTPS_ERRORS === 'true';

export default defineConfig({
  testDir: './tests/specs',
  testMatch: '**/*.spec.ts',
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: retries,
  workers: 1,
  reportSlowTests: null,
  reporter: [
    ['html', { outputFolder: 'test-results/html-report', open: 'never' }],
    ['json', { outputFile: 'test-results/results.json' }],
    ['junit', { outputFile: 'test-results/junit.xml' }],
  ],

  use: {
    baseURL: baseURL,
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    ignoreHTTPSErrors: ignoreHTTPSErrors,
  },

  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'], headless },
    },
  ],

  webServer: undefined,
  timeout: testTimeout,
  globalTimeout: testTimeout * 10,
  expect: {
    timeout: 5000,
  },
  outputDir: 'test-results/artifacts',
});
