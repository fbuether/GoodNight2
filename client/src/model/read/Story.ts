import {requireString} from "../Deserialise";


export interface Story {
  readonly name: string;
  readonly urlname: string;
};


export function asStory(obj: any): Story {
  return {
    name: requireString(obj["name"]),
    urlname: requireString(obj["urlname"])
  };
}
