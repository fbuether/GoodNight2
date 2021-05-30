import * as P from "../util/ProtoLens";

import {Home} from "./page/Home";
import {FinishSignIn} from "./page/user/FinishSignIn";
import {StoryOverview} from "./page/read/StoryOverview";
import {ReadStory} from "./page/read/ReadStory";
import {SelectStory} from "./page/write/SelectStory";
import {CreateStory} from "./page/write/CreateStory";
import {WriteStory} from "./page/write/WriteStory";
import {WriteScene} from "./page/write/WriteScene";
import {WriteQuality} from "./page/write/WriteQuality";
import {StartAdventure} from "./page/read/StartAdventure";


import type {Pages} from "./Pages";

import {PageDescriptor} from "../core/PageDescriptor";



const AllPages = [
  ReadStory, StartAdventure, StoryOverview,
  FinishSignIn,
  CreateStory, SelectStory, WriteQuality, WriteScene, WriteStory,
  Home
];

export interface Page {
  page: Pages;
  user: User;
}


export const Page = {
  default: Home.page,

  ofUrl: (pathname: string): PageDescriptor => {
    let page = AllPages.find(p => p.path.test(pathname));
    if (page !== undefined) {
      let matches = pathname.match(page.path);
      return page.ofUrl(pathname, matches != null ? matches : []);
    }

    return Home.page;
  }
}
