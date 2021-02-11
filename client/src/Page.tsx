import * as React from "react";

import State, {Page} from "./model/State";

import Frame from "./play/Frame";
import Start from "./pages/Start";
import Read from "./pages/Read";

function getPage(page: Page) {
  switch (page.kind) {
      case "start": return <Start {...page}></Start>;
      case "read": return <Read {...page}></Read>;

      default: let exhaust: never = page; return exhaust;
  }
}


export default function Page(state: State) {


  return (
    <div id="page" className="container-lg shadow mt-4 px-2 px-sm-3 px-md-4 py-2 py-md-3">

      <nav className="navbar navbar-expand-sm navbar-light py-0">
        <div className="container-fluid px-0">
          <span className="navbar-brand">GoodNight</span>

          <button className="navbar-toggler collapsed" type="button"
            data-bs-toggle="collapse" data-bs-target="#navbarNav"
            aria-controls="navbarNav" aria-expanded="false"
            aria-label="Toggle navigation">
            <span className="navbar-toggler-icon"></span>
          </button>

          <div className="collapse navbar-collapse" id="navbarNav">
            <ul className="navbar-nav me-auto">
              <li className="nav-item active">
                <a className="nav-link" href="#">Home</a>
              </li>
              <li className="nav-item">
                <a className="nav-link" href="#">Features</a>
              </li>
            </ul>
          </div>
        </div>
      </nav>
      <hr className="mt-1" />

      ${getPage(state.page)}
    </div>
  );
}
