

export interface Player {
  name: string;
  history: Array<Action>;
  current: Scene;
  state: Map<string, Value>;
}

