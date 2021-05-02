import * as P from "../util/ProtoLens";

import type {Home} from "./page/Home";
import type {SignIn} from "./page/user/SignIn";
import type {StoryOverview} from "./page/read/StoryOverview";
import type {ReadStory} from "./page/read/ReadStory";


export type Pages =
    | Home
    | SignIn
    | StoryOverview
    | ReadStory;


let guardHome = (a: Pages): a is Home => (a.page == "Home");
let guardSignIn = (a: Pages): a is SignIn => (a.page == "SignIn");
let guardStoryOverview = (a: Pages): a is StoryOverview => (a.page == "StoryOverview");
let guardReadStory = (a: Pages): a is ReadStory => (a.page == "ReadStory");

export const Lens = P.id<Pages>()
  .prop("page")
  .union("Home", guardHome, lens => lens)
  .union("SignIn", guardSignIn, lens => lens)
  .union("StoryOverview", guardStoryOverview, lens => lens
    .prop("page")
    .path("stories", lens => lens
      .prop("state")))
  .union("ReadStory", guardReadStory, lens => lens
    .prop("urlname")
    .prop("story"))
