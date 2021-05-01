import * as P from "../ProtoLens";

import type {Home} from "../page/Home";
import type {SignIn} from "../page/user/SignIn";
import type {StoryOverview} from "../page/read/StoryOverview";


export type PageState =
    | Home
    | SignIn
    | StoryOverview;



let guardHome = (a: PageState): a is Home => (a.page == "Home");
let guardSignIn = (a: PageState): a is SignIn => (a.page == "SignIn");
let guardStoryOverview = (a: PageState): a is StoryOverview => (a.page == "StoryOverview");


export const Lens = P.id<PageState>()
  .prop("page")
  .union("Home", guardHome, lens => lens)
  .union("SignIn", guardSignIn, lens => lens)
  .union("StoryOverview", guardStoryOverview, lens => lens
    .prop("page")
    .path("stories", lens => lens
      .prop("state")))
;

