
export enum QualityType {
  Int = 1,
  Bool = 2,
  Enum = 3
}

export interface Quality {
  name: string;
  type: QualityType;
  icon?: string;
  description: string;
  scene?: string;
}
