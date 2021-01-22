import * as React from "react";

import Frame from "./play/Frame";


export default function Page(page: {}) {
  return (
    <div id="page" className="container-lg shadow-sm mt-lg-4 px-0">
      <nav id="nav" className="navbar navbar-expand-sm navbar-light py-1">
        <h1 className="navbar-brand mb-0">GoodNight</h1>
        <button className="navbar-toggler" type="button"
          data-toggle="collapse" data-target="#navbarNav"
          aria-controls="navbarNav" aria-expanded="false"
          aria-label="Toggle navigation">
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav">
            <li className="nav-item mx-1 px-1 active">
              <a className="nav-link" href="#">Home</a>
            </li>
            <li className="nav-item mx-1 px-1">
             <a className="nav-link" href="#">Features</a>
            </li>
          </ul>
        </div>
      </nav>
      <Frame></Frame>
    </div>
  );
}
