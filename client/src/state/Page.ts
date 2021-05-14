import * as P from "../util/ProtoLens";

import {Home} from "./page/Home";
import {SignIn} from "./page/user/SignIn";
import {StoryOverview} from "./page/read/StoryOverview";
import {ReadStory} from "./page/read/ReadStory";
import {SelectStory} from "./page/write/SelectStory";
import {CreateStory} from "./page/write/CreateStory";
import {WriteStory} from "./page/write/WriteStory";
import {WriteScene} from "./page/write/WriteScene";
import {WriteQuality} from "./page/write/WriteQuality";
import {StartAdventure} from "./page/read/StartAdventure";


import type {Page as PageState} from "../components/Page";
import type {Pages} from "./Pages";

import {PageDescriptor} from "../core/PageDescriptor";


export type Page = PageState;


const AllPages = [
  Home, SignIn, StoryOverview, ReadStory, SelectStory, CreateStory, WriteStory,
  WriteScene, WriteQuality, StartAdventure
];



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
