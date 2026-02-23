/**
 * Authentication helpers
 * Provides utilities for token handling and authorization
 */

export interface TokenPayload {
  [key: string]: any;
}

/**
 * Extract bearer token from Authorization header
 * @param authHeader Authorization header value (e.g., "Bearer token123")
 * @returns Token string without "Bearer " prefix
 */
export function extractBearerToken(authHeader: string): string {
  const match = authHeader.match(/^Bearer\s+(.+)$/i);
  return match ? match[1] : '';
}

/**
 * Decode JWT token payload (without verification)
 * Note: This is strictly for test inspection - does not verify signature
 * @param token JWT token
 * @returns Decoded payload
 */
export function decodeToken(token: string): TokenPayload {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) {
      throw new Error('Invalid JWT format');
    }

    const payload = parts[1];
    const decoded = Buffer.from(payload, 'base64').toString('utf-8');
    return JSON.parse(decoded);
  } catch (error) {
    console.error('Failed to decode token:', error);
    return {};
  }
}

/**
 * Validate JWT token structure (basic validation, no signature check)
 * @param token JWT token to validate
 * @returns true if token has valid JWT structure
 */
export function isValidToken(token: string): boolean {
  if (!token || typeof token !== 'string') {
    return false;
  }

  const parts = token.split('.');
  return parts.length === 3 && parts.every((part) => part.length > 0);
}

/**
 * Extract claim value from token
 * @param token JWT token
 * @param claimName Name of the claim to extract
 * @returns Claim value or undefined if not found
 */
export function getTokenClaim(token: string, claimName: string): any {
  const payload = decodeToken(token);
  return payload[claimName];
}

/**
 * Create authorization header value
 * @param token Bearer token
 * @returns Authorization header value (e.g., "Bearer token123")
 */
export function createAuthHeader(token: string): string {
  return `Bearer ${token}`;
}
