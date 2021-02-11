import Link from "./Link";


export default function Navigation(state: {}) {
  return (
    <nav class="navbar navbar-expand-sm navbar-light py-0">
      <div class="container-fluid px-0">
        <span class="navbar-brand">GoodNight</span>

        <button class="navbar-toggler collapsed" type="button"
          data-bs-toggle="collapse" data-bs-target="#navbarNav"
          aria-controls="navbarNav" aria-expanded="false"
          aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav me-auto">
            <li class="nav-item active">
              <Link class="nav-link" href="/home">Home</Link>
            </li>
            <li class="nav-item">
              <Link class="nav-link" href="/read">Read</Link>
            </li>
            <li class="nav-item">
              <Link class="nav-link" href="https://github.com/fbuether/GoodNight2">Github</Link>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
}
