import type {Key} from "../Key";

export interface Quality {
  name: string;
  story: string;
  urlname: string;
  key: string;

  icon: string | null;
  raw: string;
  tags: Array<string>;
  category: Array<string>;

  inLinks: Array<Key>;
}
