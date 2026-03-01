/**
 * Custom fetch client that handles HTTPS certificate errors
 * Useful for testing environments with self-signed certificates
 */

import https from 'https';

// Disable certificate verification globally for self-signed certificates in test environments
if (process.env.IGNORE_HTTPS_ERRORS === 'true' || process.env.NODE_TLS_REJECT_UNAUTHORIZED === '0') {
  https.globalAgent.options.rejectUnauthorized = false;
}

/**
 * Get fetch options with optional HTTPS agent for self-signed certs
 */
function getFetchOptions(): RequestInit {
  const options: RequestInit = {};

  // Ignore HTTPS errors if explicitly enabled or if NODE_TLS_REJECT_UNAUTHORIZED is disabled
  const ignoreErrors =
    process.env.IGNORE_HTTPS_ERRORS === 'true' ||
    process.env.NODE_TLS_REJECT_UNAUTHORIZED === '0';

  if (ignoreErrors) {
    // Use the global HTTPS agent that was configured above
    // @ts-ignore - Node.js fetch has agent support
    options.agent = https.globalAgent;
  }

  return options;
}

/**
 * Wrapper around fetch that respects IGNORE_HTTPS_ERRORS
 */
export async function customFetch(
  url: string,
  init?: RequestInit,
): Promise<Response> {
  const options = {
    ...getFetchOptions(),
    ...init,
  };

  return fetch(url, options);
}
