import * as P from "../util/ProtoLens";

import type {Home} from "./page/Home";
import type {SignIn} from "./page/user/SignIn";
import type {StoryOverview} from "./page/read/StoryOverview";
import type {ReadStory} from "./page/read/ReadStory";
import type {SelectStory} from "./page/write/SelectStory";
import type {WriteStory} from "./page/write/WriteStory";

export type Pages =
    | Home
    | SignIn
    | StoryOverview
    | ReadStory
    | SelectStory
    | WriteStory;


let guardHome = (a: Pages): a is Home => (a.page == "Home");
let guardSignIn = (a: Pages): a is SignIn => (a.page == "SignIn");
let guardStoryOverview = (a: Pages): a is StoryOverview => (a.page == "StoryOverview");
let guardReadStory = (a: Pages): a is ReadStory => (a.page == "ReadStory");
let guardSelectStory = (a: Pages): a is SelectStory => (a.page == "SelectStory");
let guardWriteStory = (a: Pages): a is WriteStory => (a.page == "WriteStory");

export const Lens = P.id<Pages>()
  .prop("page")
  .union("Home", guardHome, lens => lens)
  .union("SignIn", guardSignIn, lens => lens)
  .union("StoryOverview", guardStoryOverview, lens => lens
    .prop("page")
    .prop("stories"))
  .union("ReadStory", guardReadStory, lens => lens
    .prop("urlname")
    .prop("story"))
  .union("SelectStory", guardSelectStory, lens => lens
    .prop("stories"))
  .union("WriteStory", guardWriteStory, lens => lens
    .prop("urlname")
    .prop("story")
    .prop("categories"));
