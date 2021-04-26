import {requireString} from "../Deserialise";


export interface Story {
  name: string;
  urlname: string;

  description: string;
};


export function asStory(obj: any): Story {
  return {
    name: requireString(obj["name"]),
    urlname: requireString(obj["urlname"]),
    description: requireString(obj["description"])
  };
}
