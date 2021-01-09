using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpu_snatcher.Helpers
{
    public static class InjectionScripts
    {
        public const string CanadaComputersGetAll =
            @"() => {
                const selectors = Array.from(document.querySelectorAll('#product-list>.toggleBox'));
                const availablesStores = ['Online Store', 'Gatineau', 'Ottawa Orleans', 'Downtown Ottawa', 'Ottawa Merivale', 'Kanata'];
                return selectors.map(se => {
                    const availabilityEl = se.previousElementSibling;

                    const availableCities = Array.from(availabilityEl.querySelectorAll('.col-md-4.col-sm-6'))
                    .map(ae => {
                        nameEl = ae.querySelector('.col-9>p');
                        stockEl = ae.querySelector('.stocknumber>strong');
                        return  {
                            name: nameEl ? nameEl.innerHTML : '',
                            stock: stockEl ? stockEl.innerHTML : '-'
                        }
                    })
                    .filter(c => c.stock !== '-' && availablesStores.includes(c.name))
        
                    return {
                        title: se.querySelector('.productTemplate_title > a').innerHTML,
                        imageUrl: se.querySelector('.pq-img-manu_logo').src,
                        pageUrl: se.querySelector('.productTemplate_title > a').href,
                        price: parseFloat(se.querySelector('.pq-hdr-product_price > strong').innerHTML.substring(1)),
                        available: availableCities.length > 0
                    }
                });
            }";

    }
}
