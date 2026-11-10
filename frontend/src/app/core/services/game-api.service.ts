import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { GameState } from '../models/game-state.model';

@Injectable({ providedIn: 'root' })
export class GameApiService {
  private readonly http = inject(HttpClient);

  readonly state = signal<GameState | null>(null);
  readonly error = signal<string | null>(null);

  load(): void {
    this.http.get<GameState>('/api/game/state').subscribe({
      next: (s) => this.apply(s),
      error: () => this.error.set("Impossible de joindre l'API."),
    });
  }

  click(): void {
    this.http.post<GameState>('/api/game/click', {}).subscribe({
      next: (s) => this.apply(s),
      error: () => this.error.set("Impossible de joindre l'API."),
    });
  }

  private apply(s: GameState): void {
    this.state.set(s);
    this.error.set(null);
  }
}