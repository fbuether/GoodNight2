import type {Key} from "../Key";

export interface Scene {
  name: string;
  urlname: string;
  key: string;

  raw: string;
  tags: Array<string>;
  category: Array<string>;

  outLinks: Array<Key>;
  inLinks: Array<Key>;
  qualities: Array<Key>;
}
