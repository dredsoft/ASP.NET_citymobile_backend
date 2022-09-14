var violationPricing;
(function ($) {

   violationPricing = new Vue({
       el: '#event-violation-pricing',
        components: {

        },
       mounted: function () {
           var self = this;
           ajaxService("GET", objEvent.GetPricing, null, function (data) {
               self.eventPricing = data.data;
           });
        },
       data: {
           
           violations: objEvent.violations,
           eventPricing: [],
           violationId: '',
           fee: null,
           isUpdate: false,
        },
        computed: {

        },
       methods: {
           CreatePrice: function () {
               var self = this;
               var violation = _.find(objEvent.violations, function (num) {
                   return num.Id === self.violationId;
               });

               blockUI();
               var data = { EventId: objEvent.EventId, ViolationId: self.violationId, Fee: self.fee };
               ajaxService("POST", objEvent.AddPricing, JSON.stringify(data),
                   function (data) { //Success
                       data.data.violation = { name: violation.Name };
                       self.eventPricing.push(data.data);

                       self.violationId = null;
                       self.fee = null;
                       demo.showNotification("success", "Price added.");

                       unblockUI();
                   },
                   function () {
                       unblockUI();
                   });

           },

           RemovePrice: function (id) {
               var self = this;
               var url = objEvent.DeletePricing + "/" + id;
               blockUI();
               ajaxService("GET", url, null, function (data) {

                   demo.showNotification("success", "Price deleted.");

                   //Remove from UI
                   var deletedItemIndex = _.findIndex(self.eventPricing, function (price) {
                       return (price.id == id);
                   });

                   self.eventPricing.splice(deletedItemIndex, 1);

                   unblockUI();
               });
           }
        }
    });

}(jQuery))