import { generateUUID, getSessionId, clearSessionId } from './main';

describe('main UUID & session helpers', () => {
  const UUID_V4_REGEX =
    /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

  afterEach(() => {
    clearSessionId();
  });

  describe('generateUUID', () => {
    it('should generate a valid UUID v4 using crypto.randomUUID when available', () => {
      const uuid = generateUUID();
      expect(uuid).toMatch(UUID_V4_REGEX);
    });

    it('should generate unique UUIDs on subsequent calls', () => {
      const uuid1 = generateUUID();
      const uuid2 = generateUUID();
      expect(uuid1).not.toEqual(uuid2);
      expect(uuid1).toMatch(UUID_V4_REGEX);
      expect(uuid2).toMatch(UUID_V4_REGEX);
    });

    it('should fall back to crypto.getRandomValues when crypto.randomUUID is not a function', () => {
      const originalRandomUUID = crypto.randomUUID;
      try {
        // Simulate non-secure context where crypto.randomUUID is undefined
        Object.defineProperty(crypto, 'randomUUID', {
          value: undefined,
          configurable: true,
          writable: true,
        });

        const uuid = generateUUID();
        expect(uuid).toMatch(UUID_V4_REGEX);
      } finally {
        Object.defineProperty(crypto, 'randomUUID', {
          value: originalRandomUUID,
          configurable: true,
          writable: true,
        });
      }
    });

    it('should fall back to Math.random when both crypto.randomUUID and crypto.getRandomValues are unavailable', () => {
      const originalRandomUUID = crypto.randomUUID;
      const originalGetRandomValues = crypto.getRandomValues;
      try {
        Object.defineProperty(crypto, 'randomUUID', {
          value: undefined,
          configurable: true,
          writable: true,
        });
        Object.defineProperty(crypto, 'getRandomValues', {
          value: undefined,
          configurable: true,
          writable: true,
        });

        const uuid = generateUUID();
        expect(uuid).toMatch(UUID_V4_REGEX);
      } finally {
        Object.defineProperty(crypto, 'randomUUID', {
          value: originalRandomUUID,
          configurable: true,
          writable: true,
        });
        Object.defineProperty(crypto, 'getRandomValues', {
          value: originalGetRandomValues,
          configurable: true,
          writable: true,
        });
      }
    });

    it('should fall back to Math.random when crypto.getRandomValues throws', () => {
      const originalRandomUUID = crypto.randomUUID;
      const originalGetRandomValues = crypto.getRandomValues;
      try {
        Object.defineProperty(crypto, 'randomUUID', {
          value: undefined,
          configurable: true,
          writable: true,
        });
        Object.defineProperty(crypto, 'getRandomValues', {
          value: () => {
            throw new Error('QuotaExceededError');
          },
          configurable: true,
          writable: true,
        });

        const uuid = generateUUID();
        expect(uuid).toMatch(UUID_V4_REGEX);
      } finally {
        Object.defineProperty(crypto, 'randomUUID', {
          value: originalRandomUUID,
          configurable: true,
          writable: true,
        });
        Object.defineProperty(crypto, 'getRandomValues', {
          value: originalGetRandomValues,
          configurable: true,
          writable: true,
        });
      }
    });
  });

  describe('getSessionId', () => {
    it('should generate and persist a new session ID when none exists', () => {
      clearSessionId();
      const sessionId = getSessionId();
      expect(sessionId).toMatch(UUID_V4_REGEX);
      expect(sessionStorage.getItem('game_manager_session_id')).toEqual(
        sessionId,
      );
    });

    it('should return the existing session ID if already stored', () => {
      sessionStorage.setItem('game_manager_session_id', 'test-session-id-123');
      const sessionId = getSessionId();
      expect(sessionId).toEqual('test-session-id-123');
    });

    it('should generate a new session ID when getSessionId is called after clearSessionId', () => {
      const sessionId1 = getSessionId();
      clearSessionId();
      const sessionId2 = getSessionId();
      expect(sessionId1).not.toEqual(sessionId2);
    });
  });
});
