

export enum QualityType {
  Int = 1,
  Bool = 2,
  Enum = 3
}


// todo: consider different types of qualities in interface.

export interface Quality {
  name: string;
  urlname: string;

  raw: string;

  type: QualityType;
  description: string;
  hidden: boolean;
  scene: string | null;

  tags: Array<string>;
  category: Array<string>;

  // maximum: number | null;
  // minimum: number | null;
}
