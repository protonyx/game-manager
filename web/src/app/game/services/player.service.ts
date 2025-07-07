import { Injectable } from '@angular/core';
import { Player } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {

  // Get player color based on player ID (for visual indicators)
  getPlayerColor(player?: Player): string {
    if (!player) return '#cccccc'; // Default gray

    // Generate a color based on player ID for consistency
    const hash = player.id.split('').reduce((acc, char) => {
      return char.charCodeAt(0) + ((acc << 5) - acc);
    }, 0);

    // Generate a hue between 0 and 360
    const hue = hash % 360;

    // Return HSL color with fixed saturation and lightness
    return `hsl(${hue}, 70%, 60%)`;
  }
}
