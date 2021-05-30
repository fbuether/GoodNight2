import {UserService} from "../service/UserService";
import {PageDescriptor, getPageMappers} from "../core/PageDescriptor";

import type {User} from "./User";
import type {Pages} from "./Pages";

import {Home} from "./page/Home";
import {RequireSignIn} from "./page/user/RequireSignIn";


export interface Page {
  page: Pages;
  user: User;
}


export const Page = {
  default: Home.page,

  ofUrl: (pathname: string): PageDescriptor => {
    let page = getPageMappers().find(p => p.path.test(pathname));
    if (page === undefined) {
      return Home.page;
    }

    let matches = pathname.match(page.path);
    let pageDesc = page.ofUrl(matches != null ? matches : []);

    if (pageDesc.requiresAuth && UserService.get().getUserQuick() === null) {
      return RequireSignIn.forUrl(pathname);
    }

    return pageDesc;
  },


  authCheck: (desc: PageDescriptor) => {
    if (desc.requiresAuth) {
      if (UserService.get().getUserQuick() === null) {
        return RequireSignIn.forUrl(desc.url);
      }
    }

    return desc;
  }
}
