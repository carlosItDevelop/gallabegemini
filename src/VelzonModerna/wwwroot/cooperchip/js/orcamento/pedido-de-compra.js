// Initialize Bootstrap components
var orderModal = new bootstrap.Modal(document.getElementById('orderModal'));
var editItemModal = new bootstrap.Modal(document.getElementById('editItemModal'));
var newOrderOffcanvas = new bootstrap.Offcanvas(document.getElementById('newOrderOffcanvas'));

// Toggle item form
document.getElementById('addItemBtn').addEventListener('click', function () {
    document.getElementById('itemForm').classList.add('show');
});

function hideItemForm() {
    document.getElementById('itemForm').classList.remove('show');
}

// Open new order offcanvas
document.getElementById('newOrderBtn').addEventListener('click', function () {
    document.getElementById('backdrop').classList.add('show');
    newOrderOffcanvas.show();
});

// Close offcanvas
function closeOffcanvas() {
    document.getElementById('backdrop').classList.remove('show');
    newOrderOffcanvas.hide();
}

// Close modal
function closeModal() {
    orderModal.hide();
}

// View order modal
function viewOrder(orderNumber) {
    // In a real app, we would fetch order details based on orderNumber
    document.getElementById('modalOrderNumber').textContent = `Pedido ${orderNumber}`;

    // Update buttons based on status
    const status = document.getElementById('modalStatus').textContent.trim();
    const sendApprovalBtn = document.getElementById('sendApprovalBtn');
    const registerReceiptBtn = document.getElementById('registerReceiptBtn');

    sendApprovalBtn.style.display = status === 'Em Elaboração' ? 'block' : 'none';
    registerReceiptBtn.style.display = (status === 'Em Elaboração' || status === 'Cancelado') ? 'none' : 'block';

    orderModal.show();
}

// Show all history items
function showAllHistory() {
    // In a real app, we would fetch all history items here
    Swal.fire({
        title: 'Histórico Completo',
        html: `
                    <div class="text-left" style="max-height: 24rem; overflow-y: auto;">
                        <div class="d-flex align-items-start mb-3">
                            <div class="flex-shrink-0 bg-primary rounded-circle p-2 me-3">
                                <i class="fas fa-check text-white"></i>
                            </div>
                            <div>
                                <p class="fw-medium mb-0">Pedido recebido totalmente</p>
                                <p class="text-muted small">25/05/2023 14:30 - Por João Silva</p>
                            </div>
                        </div>
                        <div class="d-flex align-items-start mb-3">
                            <div class="flex-shrink-0 bg-purple rounded-circle p-2 me-3">
                                <i class="fas fa-truck text-white"></i>
                            </div>
                            <div>
                                <p class="fw-medium mb-0">Pedido enviado pelo fornecedor</p>
                                <p class="text-muted small">20/05/2023 09:15 - Por Fornecedor A</p>
                            </div>
                        </div>
                        <div class="d-flex align-items-start mb-3">
                            <div class="flex-shrink-0 bg-success rounded-circle p-2 me-3">
                                <i class="fas fa-thumbs-up text-white"></i>
                            </div>
                            <div>
                                <p class="fw-medium mb-0">Pedido aprovado</p>
                                <p class="text-muted small">12/05/2023 16:45 - Por Maria Souza (Gerente)</p>
                            </div>
                        </div>
                        <div class="d-flex align-items-start mb-3">
                            <div class="flex-shrink-0 bg-warning rounded-circle p-2 me-3">
                                <i class="fas fa-paper-plane text-white"></i>
                            </div>
                            <div>
                                <p class="fw-medium mb-0">Pedido enviado para aprovação</p>
                                <p class="text-muted small">10/05/2023 17:30 - Por João Silva</p>
                            </div>
                        </div>
                        <div class="d-flex align-items-start mb-3">
                            <div class="flex-shrink-0 bg-secondary rounded-circle p-2 me-3">
                                <i class="fas fa-file-alt text-white"></i>
                            </div>
                            <div>
                                <p class="fw-medium mb-0">Pedido criado</p>
                                <p class="text-muted small">10/05/2023 14:00 - Por João Silva</p>
                            </div>
                        </div>
                        <div class="d-flex align-items-start">
                            <div class="flex-shrink-0 bg-info rounded-circle p-2 me-3">
                                <i class="fas fa-comment text-white"></i>
                            </div>
                            <div>
                                <p class="fw-medium mb-0">Observação adicionada</p>
                                <p class="text-muted small">10/05/2023 14:05 - Por João Silva</p>
                                <p class="fst-italic small">"Entregar na recepção do prédio A"</p>
                            </div>
                        </div>
                    </div>
                `,
        background: '#1f2937',
        color: '#f3f4f6',
        confirmButtonColor: '#3b82f6',
        width: '600px'
    });
}

// Confirm send for approval
function confirmSendForApproval() {
    Swal.fire({
        title: 'Enviar para Aprovação',
        text: 'Tem certeza que deseja enviar este pedido para aprovação? Esta ação não pode ser desfeita.',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3b82f6',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sim, enviar!',
        cancelButtonText: 'Cancelar',
        background: '#1f2937',
        color: '#f3f4f6'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Enviado!',
                text: 'O pedido foi enviado para aprovação com sucesso.',
                icon: 'success',
                background: '#1f2937',
                color: '#f3f4f6'
            });
        }
    });
}

// Cancel order confirmation
function cancelOrder(orderNumber) {
    Swal.fire({
        title: 'Cancelar Pedido',
        text: `Tem certeza que deseja cancelar o pedido ${orderNumber}? Esta ação não pode ser desfeita.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sim, cancelar!',
        cancelButtonText: 'Não, manter',
        background: '#1f2937',
        color: '#f3f4f6'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Cancelado!',
                text: `O pedido ${orderNumber} foi cancelado.`,
                icon: 'success',
                background: '#1f2937',
                color: '#f3f4f6'
            });
        }
    });
}

// Register receipt
function registerReceipt(orderNumber) {
    Swal.fire({
        title: 'Registrar Recebimento',
        html: `
                    <div class="text-left">
                        <div class="mb-3">
                            <label class="form-label">Tipo de Recebimento</label>
                            <select id="receiptType" class="form-select">
                                <option value="partial">Parcial</option>
                                <option value="total">Total</option>
                            </select>
                        </div>
                        <div id="partialFields">
                            <div class="mb-3">
                                <label class="form-label">Quantidade Recebida</label>
                                <input type="number" id="receivedQty" class="form-control">
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Nota Fiscal</label>
                                <input type="text" id="invoiceNumber" class="form-control">
                            </div>
                        </div>
                    </div>
                `,
        showCancelButton: true,
        confirmButtonText: 'Registrar',
        cancelButtonText: 'Cancelar',
        background: '#1f2937',
        color: '#f3f4f6',
        preConfirm: () => {
            const type = document.getElementById('receiptType').value;
            const qty = document.getElementById('receivedQty').value;
            const invoice = document.getElementById('invoiceNumber').value;

            if (type === 'partial' && (!qty || qty <= 0)) {
                Swal.showValidationMessage('Informe a quantidade recebida');
            }

            return { type, qty, invoice };
        }
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Sucesso!',
                text: 'Recebimento registrado com sucesso.',
                icon: 'success',
                background: '#1f2937',
                color: '#f3f4f6'
            });
        }
    });
}

// Edit item modal functions
function editItem(productName, quantity, unitPrice) {
    document.getElementById('editProductName').value = productName;
    document.getElementById('editQuantity').value = quantity;
    document.getElementById('editUnitPrice').value = `R$ ${unitPrice.toFixed(2).replace('.', ',')}`;
    document.getElementById('editTotalPrice').value = `R$ ${(quantity * unitPrice).toFixed(2).replace('.', ',')}`;

    editItemModal.show();
}

function closeEditItemModal() {
    editItemModal.hide();
}

function saveItemChanges() {
    const productName = document.getElementById('editProductName').value;
    const quantity = document.getElementById('editQuantity').value;
    const unitPrice = document.getElementById('editUnitPrice').value;

    // Here you would save the changes to your data/API
    Swal.fire({
        title: 'Item Atualizado!',
        text: `O item ${productName} foi atualizado com sucesso.`,
        icon: 'success',
        background: '#1f2937',
        color: '#f3f4f6'
    });

    closeEditItemModal();
}

function deleteItem(productName) {
    Swal.fire({
        title: 'Excluir Item',
        text: `Tem certeza que deseja excluir o item ${productName}?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Sim, excluir!',
        cancelButtonText: 'Cancelar',
        background: '#1f2937',
        color: '#f3f4f6'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Excluído!',
                text: `O item ${productName} foi removido do pedido.`,
                icon: 'success',
                background: '#1f2937',
                color: '#f3f4f6'
            });
        }
    });
}

// Close offcanvas when clicking on backdrop
document.getElementById('backdrop').addEventListener('click', closeOffcanvas);

// Calculate total price when editing
document.getElementById('editQuantity').addEventListener('input', calculateTotal);
document.getElementById('editUnitPrice').addEventListener('input', calculateTotal);

function calculateTotal() {
    const quantity = parseFloat(document.getElementById('editQuantity').value) || 0;
    const unitPriceStr = document.getElementById('editUnitPrice').value.replace('R$ ', '').replace(',', '.');
    const unitPrice = parseFloat(unitPriceStr) || 0;

    const total = quantity * unitPrice;
    document.getElementById('editTotalPrice').value = `R$ ${total.toFixed(2).replace('.', ',')}`;
}