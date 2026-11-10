import { DecimalPipe } from '@angular/common';
import { Component, OnInit, computed, inject } from '@angular/core';
import { GameApiService } from '../../core/services/game-api.service';

@Component({
  selector: 'app-game',
  imports: [DecimalPipe],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss',
})
export class GameComponent implements OnInit {
  protected readonly api = inject(GameApiService);
  protected readonly state = this.api.state;
  protected readonly croquettes = computed(() =>
    this.state()?.ressources.find((r) => r.code === 'croquettes'),
  );

  ngOnInit(): void {
    this.api.load();
  }

  ronronner(): void {
    this.api.click();
  }
}