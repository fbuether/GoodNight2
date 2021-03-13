import * as P from "../ProtoLens";


export interface ReadStoryPart {
  kind: "ReadStoryPart";
}


export const ReadStoryPart = {
  lens: <T>(id: P.Prism<T, ReadStoryPart>) => id
}
