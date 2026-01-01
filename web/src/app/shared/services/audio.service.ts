import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AudioService {
  private audio: HTMLAudioElement | null = null;
  private turnChimePath = 'assets/audio/turn-chime.mp3';

  constructor() {
    // Pre-load the audio file
    this.loadAudio();
  }

  private loadAudio(): void {
    try {
      this.audio = new Audio(this.turnChimePath);
      // Preload the audio file
      this.audio.load();
    } catch (error) {
      console.error('Error loading audio:', error);
    }
  }

  /**
   * Play the turn chime sound
   */
  playTurnChime(): void {
    try {
      if (this.audio) {
        // Reset the audio to the beginning if it's already playing
        this.audio.currentTime = 0;
        this.audio.play().catch(error => {
          console.error('Error playing audio:', error);
        });
      }
    } catch (error) {
      console.error('Error playing turn chime:', error);
    }
  }
}
