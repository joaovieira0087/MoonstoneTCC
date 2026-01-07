let ultimoEstado = null;

function inicializarLikeDislike(jogoId, token) {
    const btnLike = document.getElementById('btnLike');
    const btnDislike = document.getElementById('btnDislike');

    if (!btnLike || !btnDislike) return;

    // Buscar avaliação atual do usuário
    fetch(`/AvaliacaoJogo/EstadoLikeDislike?jogoId=${jogoId}`)
        .then(res => res.json())
        .then(data => {
            ultimoEstado = data.gostou;
            atualizarBotoes(ultimoEstado);
        });

    btnLike.addEventListener('click', () => votar(true));
    btnDislike.addEventListener('click', () => votar(false));

    function votar(gostou) {
        const mesmoValor = ultimoEstado === gostou;

        if (mesmoValor) {
            // Remover avaliação
            fetch('/AvaliacaoJogo/Remover', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({ jogoId })
            }).then(() => {
                ultimoEstado = null;
                atualizarBotoes(null);
                mostrarNotificacao("Avaliação removida", "warning");
            });
        } else {
            // Enviar nova avaliação
            fetch('/AvaliacaoJogo/VotarLikeDislike', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({ jogoId, gostou })
            }).then(() => {
                ultimoEstado = gostou;
                atualizarBotoes(gostou);
                mostrarNotificacao(
                    gostou ? "Você curtiu este jogo!" : "Você não curtiu este jogo!",
                    gostou ? "success" : "danger"
                );
            });
        }
    }

    function atualizarBotoes(gostou) {
        // Botão Like
        btnLike.classList.toggle('btn-success', gostou === true);
        btnLike.classList.toggle('btn-outline-success', gostou !== true);
        btnLike.querySelector('i').className = gostou === true ? "fas fa-thumbs-up" : "far fa-thumbs-up";

        // Botão Dislike
        btnDislike.classList.toggle('btn-danger', gostou === false);
        btnDislike.classList.toggle('btn-outline-danger', gostou !== false);
        btnDislike.querySelector('i').className = gostou === false ? "fas fa-thumbs-down" : "far fa-thumbs-down";
    }
}

function mostrarNotificacao(texto, cor = "success") {
    const noti = document.createElement("div");
    noti.className = `alert alert-${cor} position-fixed top-0 end-0 mt-3 me-3 shadow`;
    noti.style.zIndex = 9999;
    noti.style.transition = "opacity 0.5s";
    noti.innerHTML = `<i class="fas fa-check-circle me-2"></i>${texto}`;
    noti.style.opacity = 0;

    document.body.appendChild(noti);

    setTimeout(() => { noti.style.opacity = 1; }, 10);
    setTimeout(() => {
        noti.style.opacity = 0;
        setTimeout(() => noti.remove(), 500);
    }, 3000);
}
