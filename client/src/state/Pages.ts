import * as P from "../util/ProtoLens";

import type {Home} from "./page/Home";
import type {SignIn} from "./page/user/SignIn";
import type {StoryOverview} from "./page/read/StoryOverview";


export type Pages =
    | Home
    | SignIn
    | StoryOverview;


let guardHome = (a: Pages): a is Home => (a.page == "Home");
let guardSignIn = (a: Pages): a is SignIn => (a.page == "SignIn");
let guardStoryOverview = (a: Pages): a is StoryOverview => (a.page == "StoryOverview");

export const Lens = P.id<Pages>()
  .prop("page")
  .union("Home", guardHome, lens => lens)
  .union("SignIn", guardSignIn, lens => lens)
  .union("StoryOverview", guardStoryOverview, lens => lens
    .prop("page")
    .path("stories", lens => lens
      .prop("state")))
