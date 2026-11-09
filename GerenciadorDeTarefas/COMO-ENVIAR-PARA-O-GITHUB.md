# Como enviar para o GitHub

## 1. Crie o repositório
No site do GitHub: **New repository**, nome `GerenciadorDeTarefas`.
Não marque "Add a README" nem ".gitignore" (o projeto já tem os dois).
Deixe **Public**, ou **Private** e depois convide o professor em *Settings > Collaborators*.

## 2. Envie o código
Abra o terminal **dentro da pasta GerenciadorDeTarefas** e rode:

```bash
git init
git add .
git commit -m "Projeto inicial: API de tarefas e front-end React"
git branch -M main
git remote add origin https://github.com/SEU-USUARIO/GerenciadorDeTarefas.git
git push -u origin main
```

Troque `SEU-USUARIO` pelo seu usuário do GitHub.

## 3. Depois, para atualizar
```bash
git add .
git commit -m "Descreva o que mudou"
git push
```

## 4. Mande o link ao professor
`https://github.com/SEU-USUARIO/GerenciadorDeTarefas`

Antes de enviar, confira no GitHub se NÃO apareceram as pastas `bin`, `obj` e `node_modules`
nem o arquivo `tarefas.db`. O `.gitignore` já cuida disso.
