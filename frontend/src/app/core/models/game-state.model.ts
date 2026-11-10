export interface RessourceEtat {
  code: string;
  nom: string;
  icone: string;
  quantite: number;
  productionParSeconde: number;
}

export interface GameState {
  pseudo: string;
  nombreClics: number;
  palierNom: string;
  palierTheme: string;
  ressources: RessourceEtat[];
}