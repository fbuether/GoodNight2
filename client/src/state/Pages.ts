import * as P from "../util/ProtoLens";

import {Loadable} from "./Loadable";

import type {Home} from "./page/Home";
import type {SignIn} from "./page/user/SignIn";
import type {StoryOverview} from "./page/read/StoryOverview";
import type {ReadStory} from "./page/read/ReadStory";
import type {SelectStory} from "./page/write/SelectStory";
import type {CreateStory} from "./page/write/CreateStory";
import type {WriteStory} from "./page/write/WriteStory";
import type {WriteScene} from "./page/write/WriteScene";
import type {WriteQuality} from "./page/write/WriteQuality";
import type {StartAdventure} from "./page/read/StartAdventure";

export type Pages =
    | Home
    | SignIn
    | StoryOverview
    | ReadStory
    | SelectStory
    | CreateStory
    | WriteStory
    | WriteScene
    | WriteQuality
    | StartAdventure;


let guardHome = (a: Pages): a is Home => (a.page == "Home");
let guardSignIn = (a: Pages): a is SignIn => (a.page == "SignIn");
let guardStoryOverview = (a: Pages): a is StoryOverview => (a.page == "StoryOverview");
let guardReadStory = (a: Pages): a is ReadStory => (a.page == "ReadStory");
let guardSelectStory = (a: Pages): a is SelectStory => (a.page == "SelectStory");
let guardCreateStory = (a: Pages): a is CreateStory => (a.page == "CreateStory");
let guardWriteStory = (a: Pages): a is WriteStory => (a.page == "WriteStory");
let guardWriteScene = (a: Pages): a is WriteScene => (a.page == "WriteScene");
let guardWriteQuality = (a: Pages): a is WriteQuality => (a.page == "WriteQuality");
let guardStartAdventure = (a: Pages): a is StartAdventure => (a.page == "StartAdventure");

const guardNotNull = <T>(a: T | null): a is T => (a !== null);

export const Lens = P.id<Pages>()
  .prop("page")
  .union("Home", guardHome, lens => lens)
  .union("SignIn", guardSignIn, lens => lens)
  .union("StoryOverview", guardStoryOverview, lens => lens
    .prop("page")
    .prop("stories"))
  .union("ReadStory", guardReadStory, lens => lens
    .prop("story")
    .path("adventure", Loadable.lens))
  .union("SelectStory", guardSelectStory, lens => lens
    .prop("stories"))
  .union("CreateStory", guardCreateStory, lens => lens
    .prop("name")
    .prop("isSaving"))
  .union("WriteStory", guardWriteStory, lens => lens
    .prop("urlname")
    .prop("story")
    .prop("category"))
  .union("WriteScene", guardWriteScene, lens => lens
    .path("story", Loadable.lensP)
    .path("scene", lens => lens
      .union("value", guardNotNull, Loadable.lensP))
    .prop("raw")
    .prop("isSaving"))
  .union("WriteQuality", guardWriteQuality, lens => lens
    .path("story", Loadable.lensP)
    .path("quality", lens => lens
      .union("value", guardNotNull, Loadable.lensP))
    .prop("raw")
    .prop("isSaving"))
  .union("StartAdventure", guardStartAdventure, lens => lens
    .path("story", Loadable.lensP)
    .prop("error")
    .prop("isStarting")
    .prop("name"));
