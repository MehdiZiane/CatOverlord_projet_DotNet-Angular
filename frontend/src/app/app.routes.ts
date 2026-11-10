import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'jeu' },
  {
    path: 'jeu',
    loadComponent: () =>
      import('./features/game/game.component').then((m) => m.GameComponent),
  },
];