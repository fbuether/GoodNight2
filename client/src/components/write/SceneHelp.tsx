
export default function SceneHelp() {
  return (
    <>
      <h2>Referenz</h2>
      <p>
        Eine Szene kann verschiedene Platzhalter enthalten, um ihr Verhalten
        zu steuern.
      </p>
      <dl class="commands">
        <dt>$name: <em>name</em></dt>
        <dd>
          Legt den Namen dieser Szene fest. Muss in der Geschichte
          einzigartig sein.
        </dd>

        <dt>$start</dt>
        <dd>Diese Szene wird neuen Spielern als erstes
          angezeigt. Muss in der gesamten Geschichte genau einmal vorkommen.
        </dd>

        <dt>$show always<br />$always show</dt>
        <dd>
          Zeigt diese Szene immer als Option an, auch wenn die
          Vorbedingungen fehlen.
        </dd>

        <dt>$force show</dt>
        <dd>
          Zeigt diese Szene direkt als nächste an, wenn möglich, ohne dem
          Spieler eine Wahl zu lassen.
        </dd>

        <dt>$tag: <em>tag1</em>, <em>tag2</em>…</dt>
        <dd>
          Füge dieser Szene Tags hinzu, um sie in der Bearbeitungsansicht zu
          einem Tag mit aufzuführen.<br />
          Kein Effekt für Spieler.
        </dd>

        <dt>$category: <em>category</em>/<em>sub</em>…</dt>
        <dd>
          Ordnet diese Szene in eine Hierarchie von Kategorien ein, so dass
          sie auf der Bearbeitungsübersicht in der entsprechenden Kategorie
          angezeigt wird.<br />
          Kein Effekt für Spieler.
        </dd>
  
        <dt>$set: <em>quality</em> = <em>expr</em></dt>
        <dd>
          Setzt die Qualität eines Spielers auf den errechneten Wert, sobald
          der Spieler diese Szene ausspielt.
        </dd>

        <dt>$require: <em>expr</em></dt>
        <dd>
          Sperrt diese Szene und versteckt sie in einer Auswahl, solange ein
          Spieler diese berechnete Bedingung nicht erfüllt.
        </dd>
  
        <dt>$return: <em>scene</em></dt>
        <dd>
          Bietet unter diese Szene einen "Zurück"-Knopf an, um zu der
          genannten Szene zurück zu gelangen.
        </dd>

        <dt>$continue: <em>scene</em></dt>
        <dd>
          Bietet unter diese Szene einen "Weiter"-Knopf an, um mit der
          genannten Szene fortzufahren.
        </dd>

        <dt>$include: <em>scene</em></dt>
        <dd>
          Bindet den Inhalt der genannten Szene an dieser Stelle literal ein.
          Arbeitet nicht transitiv.
        </dd>
  
        <dt>$option: <em>scene</em><br />…<br />$end</dt>
        <dd>
          Erstellt eine Auswahlmöglichkeit am Ende dieser Szene. Der innere
          Inhalt wird als Text der Auswahl angezeigt. Wird die Option
          ausgewählt, erscheint die genannte Szene als nächstes.
        </dd>

        <dt>$if: <em>expr</em><br />…<br />$end</dt>
        <dd>
          Zeigt Inhalt nur an, wenn eine errechnete Bedingung erfüllt ist.
        </dd>

        <dt>$if: <em>expr</em><br />…<br />$else<br />…<br />$end</dt>
        <dd>
          Zeigt Inhalt nur an, wenn eine errechnete Bedingung erfüllt ist,
          oder einen alternativen Inhalt, wenn nicht.
        </dd>
      </dl>
    </>
  );
}
