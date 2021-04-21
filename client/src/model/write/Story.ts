

// used for lists of many stories, e.g. during story selection.
export interface StoryHeader {
  name: string;
  urlname: string;
  description: string;
}


export interface Story {
  name: string;
  urlname: string;

  description: string; // todo: not yet present in service.
}
