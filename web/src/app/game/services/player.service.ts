import { Injectable } from '@angular/core';
import { Player } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {

  getPlayerColor(player?: Player): string {
    if (!player) return '#cccccc';

    if (player.color) return player.color;

    // Fall back to hash-based color for backwards compatibility
    const hash = player.id.split('').reduce((acc, char) => {
      return char.charCodeAt(0) + ((acc << 5) - acc);
    }, 0);
    const hue = hash % 360;
    return `hsl(${hue}, 70%, 60%)`;
  }

  // Returns #ffffff or #000000 based on background luminance for text contrast
  getTextColor(bgHex: string): string {
    const hex = bgHex.replace('#', '');
    if (hex.length !== 6) return '#ffffff';
    const r = parseInt(hex.substring(0, 2), 16) / 255;
    const g = parseInt(hex.substring(2, 4), 16) / 255;
    const b = parseInt(hex.substring(4, 6), 16) / 255;

    const toLinear = (c: number) => c <= 0.03928 ? c / 12.92 : Math.pow((c + 0.055) / 1.055, 2.4);
    const luminance = 0.2126 * toLinear(r) + 0.7152 * toLinear(g) + 0.0722 * toLinear(b);

    return luminance > 0.179 ? '#000000' : '#ffffff';
  }
}
