import { HttpClient } from '../Common/common.js';
import { ReloadSources } from '../Common/commonResources.js';

const client = new HttpClient('/Products/Product');
const reloadSource = new ReloadSources();

//----------------------------------------
//--------------- Elements ---------------
//----------------------------------------
const btnAdd = document.querySelector('.btn-added');

const containerProducts = document.querySelector('.products');

let containerCardBody = document.querySelector('.content')


//----------------------------------------
//------------- Load Product -------------
//----------------------------------------
const displayProducts = function () {
    client.get('ProductList')
        .then(data => {
            data.forEach(function () {
                const html = `
                            <tr>
                                <td>
                                <label class="checkboxs">
                                    <input type="checkbox">
                                    <span class="checkmarks"></span>
                            </label>
                            </td>
                            <td class="productimgname">
                                <a href="javascript:void(0);" class="product-img">
                                    <img src="~/Template/img/product/product1.jpg" alt="product">
                                </a>
                                <a href="javascript:void(0);">Macbook pro</a>
                            </td>
                            <td>PT001</td>
                            <td>Computers</td>
                            <td>N/D</td>
                            <td>1500.00</td>
                            <td>pc</td>
                            <td>100.00</td>
                            <td>Admin</td>
                                <td>
                                <a class="me-3" href="product-details.html">
                                    <img src="~/Template/img/icons/eye.svg" alt="img">
                                </a>
                                <a class="me-3" href="editproduct.html">
                                    <img src="~/Template/img/icons/edit.svg" alt="img">
                                </a>
                                <a class="confirm-text" href="javascript:void(0);">
                                    <img src="~/Template/img/icons/delete.svg" alt="img">
                            </a>
                        </td>
                        </tr>
                        `
                containerProducts.insertAdjacentHTML('afterbegin', html)
                reloadSource.Reload();
            })
            //console.log('Products:', data);
        })
        .catch(error => console.error(error));
}

//----------------------------------------
//------------- Event --------------------
//----------------------------------------
btnAdd.addEventListener('click', async function (e) {
    e.preventDefault();
    containerCardBody.innerHTML = "";
    await client.getHtml('LoadCreateUpdateHtmls')
        .then(html => {
            $(".content").html(html)
            //containerCardBody.insertAdjacentHTML('afterbegin', html);
            reloadSource.Reload();
        })
        .catch(error => {
            console.error('Error during GET request:', error);
        });
});
