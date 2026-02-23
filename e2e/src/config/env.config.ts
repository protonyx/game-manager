/**
 * Environment configuration for smoke tests
 * Validates and exports environment variables with sensible defaults
 */

export const config = {
  /**
   * Base URL of the application under test
   * Required: Must be provided as BASE_URL environment variable
   * Examples: http://localhost:5000, https://staging.example.com
   */
  baseUrl: process.env.BASE_URL || '',

  /**
   * Optional: Logging level (trace, debug, info, warn, error)
   */
  logLevel: (process.env.LOG_LEVEL || 'info') as 'trace' | 'debug' | 'info' | 'warn' | 'error',

  /**
   * Optional: Test timeout in milliseconds
   */
  testTimeout: parseInt(process.env.TEST_TIMEOUT || '30000', 10),

  /**
   * Optional: Number of retries for flaky tests
   */
  testRetries: parseInt(process.env.TEST_RETRIES || '1', 10),

  /**
   * Optional: Run tests in headed mode (visible browser)
   */
  headless: process.env.HEADLESS !== 'false',

  /**
   * Optional: Ignore HTTPS certificate errors (for development with self-signed certs)
   */
  ignoreHTTPSErrors: process.env.IGNORE_HTTPS_ERRORS === 'true' || process.env.NODE_TLS_REJECT_UNAUTHORIZED === '0',
};

/**
 * Validate that required environment variables are set
 */
export function validateConfig(): void {
  const errors: string[] = [];

  if (!config.baseUrl) {
    errors.push('BASE_URL environment variable is required');
  }

  if (errors.length > 0) {
    throw new Error(`Configuration validation failed:\n${errors.join('\n')}`);
  }
}

/**
 * Log the active configuration (excluding sensitive values)
 */
export function logConfig(): void {
  console.log(`
╔════════════════════════════════════════╗
║      E2E Test Configuration            ║
╠════════════════════════════════════════╣
║ Base URL:         ${config.baseUrl.padEnd(20)} ║
║ Log Level:        ${config.logLevel.padEnd(20)} ║
║ Test Timeout:     ${config.testTimeout}ms${' '.repeat(10)} ║
║ Retries:          ${config.testRetries}${' '.repeat(24)} ║
║ Headless:         ${config.headless}${' '.repeat(20)} ║
║ Ignore HTTPS:     ${config.ignoreHTTPSErrors}${' '.repeat(19)} ║
╚════════════════════════════════════════╝
  `);
}
