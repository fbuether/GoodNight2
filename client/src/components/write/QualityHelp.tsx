
export default function QualityHelp() {
  return <>
    <h2>Referenz</h2>
    <p>
      Eine Qualität kann verschiedene Platzhalter enthalten, um ihr Verhalten
      zu steuern.
    </p>
    <dl class="commands">
      <dt>$name: <em>name</em></dt>
      <dd>
        Legt den Namen dieser Qualität fest. Muss in der Geschichte
        einzigartig sein.
      </dd>

      <dt>$type: <em>bool|int|enum</em></dt>
      <dd>
        Legt den Typen dieser Qualität fest.
        <ul>
          <li><code>bool</code>: Spieler besitzt die Qualität (<code>true</code>) oder nicht (<code>false</code>).</li>
          <li><code>int</code>: Spieler besitzt einen Anzahl der Qualität. Wert 0 bedeutet, dass er sie nicht besitzt.</li>
          <li>
            <code>enum</code>: Spieler hat die Qualität mit einem von
            mehreren festgelegten Werten. Wert 0 bedeutet, dass er sie nicht besitzt.
          </li>
        </ul>
      </dd>

      <dt>$hidden</dt>
      <dd>
        Die Qualität wird nie angezeigt, auch wenn der Spieler sie besitzt.
      </dd>

      <dt>$scene: <em>scene</em></dt>
      <dd>
        Die Qualität verweist auf eine Szene, die der Spieler aus der Qualität
        heraus starten kann.
      </dd>

        <dt>$tag: <em>tag1</em> <em>tag2</em>…</dt>
        <dd>
          Füge dieser Qualität Tags hinzu, um sie in der Bearebeitungsansicht zu
          einem Tag mit aufzuführen.<br />
          Kein Effekt für Spieler.
        </dd>

        <dt>$category: <em>category</em>/<em>sub</em>…</dt>
        <dd>
          Ordnet diese Qualität in eine Hierarchie von Kategorien ein, so dass
          sie auf der Bearbeitungsübersicht in der entsprechenden Kategorie
          angezeigt wird.<br />
          Kein Effekt für Spieler.
        </dd>

      <dt>$level <em>n</em>: <em>description</em></dt>
      <dd>
        Nur für Qualitäten vom Typ <code>enum</code>: Legt die Beschreibung für
        einen bestimmten Wert dieser Qualität fest.
      </dd>

      <dt>$min: <em>num</em><br />$max: <em>num</em></dt>
      <dd>
        Nur für Qualitäten vom Typ <code>int</code>: Legt den maximalen oder
        minimalen Wert fest, den diese Qualität erhalten kann.
      </dd>

    </dl>
  </>;
}

