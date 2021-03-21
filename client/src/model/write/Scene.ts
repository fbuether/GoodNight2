
export interface Scene {
  name: string;
  urlname: string;

  raw: string;
  isStart: boolean;
  showAlways: boolean;
  forceShow: boolean;
  tags: Array<string>;
  category: Array<string>;

  sets: Array<object>;
  "return": string;
  "continue": string;

  content: Array<object>;
}
