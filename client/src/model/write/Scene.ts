
export interface Scene {
  name: string;
  urlname: string;
  key: string;

  raw: string;
  tags: Array<string>;
  category: Array<string>;

  outLinks: Array<string>;
  inLinks: Array<string>;
}
