import {Dispatch} from "../../core/Dispatch";
import {PageDescriptor} from "../../core/PageDescriptor";

import Link from "../common/Link";


export interface NavItem {
  title: string;
  page: PageDescriptor;
  current: boolean;
}

export function NavItem(state: NavItem) {
  return (
    <li class={"nav-item" + (state.current ? " active" : "")}>
      <Link class="nav-link" current={state.current}
        action={Dispatch.Page(state.page)}>
        {state.title}
      </Link>
    </li>
  );
}

