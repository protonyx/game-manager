import { Injectable } from '@angular/core';
import { PlayerCredentials } from '../models/models';
import { SESSION_STORAGE_PREFIX, getSessionId, clearSessionId } from '../../../main';
import { gameFeatureKey } from '../state/game.reducer';

export interface StoredSession {
  sessionId: string;
  credentials: PlayerCredentials;
}

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  getSessions(): StoredSession[] {
    const currentSessionId = getSessionId();
    const sessions: StoredSession[] = [];

    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i);
      if (!key || !key.startsWith(SESSION_STORAGE_PREFIX)) {
        continue;
      }

      const suffix = `_${gameFeatureKey}`;
      if (!key.endsWith(suffix)) {
        continue;
      }

      const sessionId = key.slice(SESSION_STORAGE_PREFIX.length, -suffix.length);
      if (sessionId === currentSessionId) {
        continue;
      }

      try {
        const raw = localStorage.getItem(key);
        if (!raw) continue;

        const parsed = JSON.parse(raw);
        const credentials = parsed?.credentials as PlayerCredentials | undefined;
        if (credentials?.gameId && credentials?.playerId && credentials?.token) {
          sessions.push({ sessionId, credentials });
        }
      } catch {
        // Skip malformed entries
      }
    }

    return sessions;
  }

  destroyCurrentSession(): void {
    const sessionId = getSessionId();
    const key = `${SESSION_STORAGE_PREFIX}${sessionId}_${gameFeatureKey}`;
    localStorage.removeItem(key);
    clearSessionId();
  }

  restoreSession(session: StoredSession): void {
    // Remove the old session's localStorage entry
    const oldKey = `${SESSION_STORAGE_PREFIX}${session.sessionId}_${gameFeatureKey}`;
    localStorage.removeItem(oldKey);

    // Write credentials under the current session's key
    const currentSessionId = getSessionId();
    const newKey = `${SESSION_STORAGE_PREFIX}${currentSessionId}_${gameFeatureKey}`;
    localStorage.setItem(newKey, JSON.stringify({ credentials: session.credentials }));
  }
}
