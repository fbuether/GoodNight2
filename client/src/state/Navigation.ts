import {PageDescriptor} from "../core/PageDescriptor";

import {Home} from "./page/Home";
import {StoryOverview} from "./page/read/StoryOverview";
import {SelectStory} from "./page/write/SelectStory";

import {User} from "./User";


export interface Navigation {
  user: User;
  page: string;
}


type MenuItem = [string, PageDescriptor];

let menuItems: Array<MenuItem> = [
  ["Willkommen", Home.page],
  ["Geschichten lesen", new StoryOverview()]
];

let signedInItems: Array<MenuItem> = [
  ["Geschichten schreiben", SelectStory.page]
];


export const Navigation = {
  getMenuItems: (user: User) => {
    let items = menuItems;

    if (user.kind == "SignedIn") {
      items = items.concat(signedInItems);
    }

    return items;
  }
};
