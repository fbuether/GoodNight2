import * as React from "react";

export default function Navigation(state: {}) {
  return (
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
  );
}
