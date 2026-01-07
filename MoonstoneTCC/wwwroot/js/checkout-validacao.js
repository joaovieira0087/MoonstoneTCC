document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');

    const numeroCartaoInput = document.getElementById('numeroCartao');
    const validadeCartaoInput = document.getElementById('validadeCartao');
    const cvvCartaoInput = document.getElementById('cvvCartao');
    const nomeTitularInput = document.getElementById('nomeTitular');

    if (numeroCartaoInput && validadeCartaoInput && cvvCartaoInput && nomeTitularInput) {

        // Formatar número do cartão com espaço a cada 4 dígitos
        numeroCartaoInput.addEventListener('input', function (e) {
            let input = e.target.value.replace(/\D/g, '');
            input = input.replace(/(\d{4})(?=\d)/g, '$1 ');
            input = input.substring(0, 19);
            e.target.value = input;
        });

        // Validação antes de enviar
        form.addEventListener('submit', function (e) {
            console.log("Tentando enviar o formulário para o backend...");

            const numeroCartao = numeroCartaoInput.value.replace(/\s/g, '');
            const validadeCartao = validadeCartaoInput.value;
            const cvvCartao = cvvCartaoInput.value;
            const nomeTitular = nomeTitularInput.value;

            if (!numeroCartao || numeroCartao.length !== 16) {
                alert('Por favor, insira um número de cartão válido com 16 dígitos.');
                e.preventDefault();
                return;
            }

            if (!validadeCartao || !/^(0[1-9]|1[0-2])\/\d{2}$/.test(validadeCartao)) {
                alert('Por favor, insira uma validade válida no formato MM/AA.');
                e.preventDefault();
                return;
            }

            if (!cvvCartao || !/^\d{3}$/.test(cvvCartao)) {
                alert('Por favor, insira um CVV válido com 3 dígitos.');
                e.preventDefault();
                return;
            }

            if (!nomeTitular.trim()) {
                alert('Por favor, insira o nome do titular do cartão.');
                e.preventDefault();
                return;
            }

            console.log("Formulário validado e pronto para ser enviado.");
        });
    } else {
        console.log("Campos do cartão não existem ou cartão salvo está sendo usado.");
    }
});

