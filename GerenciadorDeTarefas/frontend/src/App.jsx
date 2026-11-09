import { useState, useEffect } from 'react';
import './App.css';
import { lerProblema } from './lerProblema';

const API = 'http://localhost:5137/tarefas';

// Quantas tarefas aparecem por página.
const TAMANHO = 5;

function App() {
  const [tarefas, setTarefas] = useState([]);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState(null);

  const [titulo, setTitulo] = useState('');
  const [erroFormulario, setErroFormulario] = useState(null);

  // id da tarefa em edição; null significa "estamos criando"
  const [tarefaEditando, setTarefaEditando] = useState(null);

  const [pagina, setPagina] = useState(1);
  const [filtroTitulo, setFiltroTitulo] = useState('');

  useEffect(() => {
    buscarTarefas(1);
  }, []);

  // Busca uma página de tarefas. Sem argumento, recarrega a página atual.
  async function buscarTarefas(paginaDesejada = pagina) {
    setCarregando(true);
    setErro(null);

    const parametros = new URLSearchParams({ pagina: paginaDesejada, tamanho: TAMANHO });
    if (filtroTitulo) {
      parametros.append('tituloContem', filtroTitulo);
    }

    try {
      const resposta = await fetch(`${API}?${parametros}`);

      if (!resposta.ok) {                          // a API respondeu COM erro
        setErro(await lerProblema(resposta));
        return;
      }

      setTarefas(await resposta.json());
      setPagina(paginaDesejada);
    } catch {
      setErro('Não foi possível conectar à API.');  // NÃO houve resposta
    } finally {
      setCarregando(false);
    }
  }

  // Filtro novo sempre volta para a página 1.
  function aplicarFiltro(evento) {
    evento.preventDefault();
    buscarTarefas(1);
  }

  // POST se estamos criando, PUT se estamos editando.
  async function salvarTarefa(evento) {
    evento.preventDefault();

    const editando = tarefas.find(t => t.id === tarefaEditando);
    const corpo = { titulo, concluida: editando ? editando.concluida : false };

    const resposta = await fetch(tarefaEditando ? `${API}/${tarefaEditando}` : API, {
      method: tarefaEditando ? 'PUT' : 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(corpo)
    });

    if (!resposta.ok) {
      setErroFormulario(await lerProblema(resposta));
      return;
    }

    setTitulo('');
    setErroFormulario(null);
    setTarefaEditando(null);
    buscarTarefas();
  }

  // Marca ou desmarca como concluída (PUT com o título atual).
  async function alternarConclusao(tarefa) {
    const resposta = await fetch(`${API}/${tarefa.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ titulo: tarefa.titulo, concluida: !tarefa.concluida })
    });

    if (!resposta.ok) {
      setErro(await lerProblema(resposta));
      return;
    }

    buscarTarefas();
  }

  function iniciarEdicao(tarefa) {
    setTarefaEditando(tarefa.id);
    setTitulo(tarefa.titulo);
    setErroFormulario(null);
  }

  function cancelarEdicao() {
    setTarefaEditando(null);
    setTitulo('');
    setErroFormulario(null);
  }

  async function removerTarefa(id) {
    const resposta = await fetch(`${API}/${id}`, { method: 'DELETE' });

    if (!resposta.ok) {
      setErro(await lerProblema(resposta));
      return;
    }

    // Se a tarefa removida era a que estava em edição, limpa o formulário.
    if (tarefaEditando === id) {
      cancelarEdicao();
    }

    buscarTarefas();
  }

  return (
    <div className="pagina">
      <h1>Tarefas</h1>

      <form className="linha" onSubmit={aplicarFiltro}>
        <input
          placeholder="Filtrar por título"
          value={filtroTitulo}
          onChange={e => setFiltroTitulo(e.target.value)}
        />
        <button type="submit">Filtrar</button>
      </form>

      {carregando && <p>Carregando...</p>}
      {erro && <p className="erro">Erro: {erro}</p>}
      {!carregando && !erro && tarefas.length === 0 && <p>Nenhuma tarefa nesta página.</p>}

      <ul className="lista">
        {tarefas.map(t => (
          <li key={t.id}>
            <label className={t.concluida ? 'concluida' : ''}>
              <input
                type="checkbox"
                checked={t.concluida}
                onChange={() => alternarConclusao(t)}
              />
              {t.titulo}
            </label>
            <span className="acoes">
              <button onClick={() => iniciarEdicao(t)}>Editar</button>
              <button onClick={() => removerTarefa(t.id)}>Remover</button>
            </span>
          </li>
        ))}
      </ul>

      {/* Veio MENOS que TAMANHO? Então é a última página: "Próxima" desliga. */}
      <div className="linha paginacao">
        <button onClick={() => buscarTarefas(pagina - 1)} disabled={pagina <= 1 || carregando}>
          Anterior
        </button>
        <span>Página {pagina}</span>
        <button onClick={() => buscarTarefas(pagina + 1)} disabled={tarefas.length < TAMANHO || carregando}>
          Próxima
        </button>
      </div>

      <h2>{tarefaEditando ? 'Editar tarefa' : 'Nova tarefa'}</h2>
      <form className="linha" onSubmit={salvarTarefa}>
        <input
          placeholder="Título da tarefa"
          value={titulo}
          onChange={e => setTitulo(e.target.value)}
        />
        <button type="submit">{tarefaEditando ? 'Salvar edição' : 'Criar'}</button>
        {tarefaEditando && (
          <button type="button" onClick={cancelarEdicao}>Cancelar</button>
        )}
      </form>
      {erroFormulario && <p className="erro">{erroFormulario}</p>}
    </div>
  );
}

export default App;
