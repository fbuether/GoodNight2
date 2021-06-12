
export enum QualityType {
  Bool = 1,
  Int = 2,
  Enum = 3
}


// in the backend, this is QualityHeader, as full qualities do not transfer
// to the front end.
export interface Quality {
  name: string;
  type: QualityType;
  icon: string | null;
  description: string;
  hidden: boolean;
}
