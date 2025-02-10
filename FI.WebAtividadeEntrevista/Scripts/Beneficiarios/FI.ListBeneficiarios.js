$(document).ready(function () {
    $('#gridBeneficiarios').dataTable({
        ajax: {
            url: '/Beneficiario/BeneficiarioList', 
            type: 'POST',
            dataSrc: function (json) {
                if (json.Result === "OK") {
                    return json.Records;
                } else {
                    alert("Erro: " + json.Message);
                    return [];
                }
            }
        },
        columns: [
            { data: 'NomeBeneficiario' },
            { data: 'CPFBeneficiario' },
            {
                data: 'Id',
                render: function (data, type, row) {
                    return `
                        <a href="/ControllerName/Editar/${data}" class="btn btn-warning btn-sm">Alterar</a>
                        <a href="/ControllerName/Excluir/${data}" class="btn btn-danger btn-sm" onclick="return confirm('Tem certeza que deseja excluir?');">Excluir</a>
                    `;
                }
            }
        ]
    });
});
