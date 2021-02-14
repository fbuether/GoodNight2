import Link from "../components/common/Link";
import type {Page} from "../model/Page";

export default interface Navigation {
  readonly currentPage: Page;
  readonly user: string;
}


export default function Navigation(state: Navigation) {
  let navigations = [
    {
      title: "Start",
      to: {
        kind: "start"  as const,
        message: "okay"
      }
    },
    {
      title: "Hels Schlucht",
      to: {
        kind: "read" as const,
        story: "Hels Schlucht",
        urlname: "hels-schlucht",
        user: state.user
      }
    }
  ];

  navigations = navigations.map(nav => { return {
    ...nav,
    current: state.currentPage.kind == nav.to.kind
  }});

  let navItems = navigations.map(item => (
    <li class="nav-item {item.current ? 'active' : ''}">
      <Link class="nav-link" to={item.to}>{item.title}</Link>
    </li>));

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
            {navItems}
          </ul>
        </div>
      </div>
    </nav>
  );
}
