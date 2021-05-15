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
  ["Geschichten lesen", StoryOverview.page],
  ["Geschichten schreiben", SelectStory.page]
];



export const Navigation = {
  menuItems: menuItems
};
