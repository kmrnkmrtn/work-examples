#ifndef arutil_h_INCLUDED
#define arutil_h_INCLUDED

#include <iostream>
#include <vector>

#include <algorithm>

template <class elem>
class array_util {
    public:
        // constructors
        array_util(elem* arr_, int length_) : _arr(arr_), _length(length_) { }
        array_util(std::vector<elem>& vec_) : _vec(vec_), _length(vec_.size()) { this->_is_vec = true; _original_vec = &vec_;}

        // getters
        elem* get_arr() const { return _arr; }
        std::vector<elem> get_vec() const  { return _vec; }
        int length() const { return _length; }
        bool is_vec() const {return _is_vec; }

        //setters
        //void set_length(int l)  { _length = l; }

        //  functions

        bool equals(const array_util<elem>& param) const
        {

            if(this->length() != param.length() ) return false;
            if(this == &param) return true;
            
            for(int i = 0; i < this->length() ; ++i)
            {
                if( (*this)[i] != param[i]) return false;
            }   
            return true;
        }

        void copy(const array_util& param)
        {
            int N = std::min(this->length(), param.length());

            if(this->is_vec())
            {
                for(int i= 0; i < N ; ++i)
                {
                        *(this->_original_vec->begin()+i) = param[i];
                }
                return;
            }
            for(int i= 0; i < N ; ++i)
            {
                *(this->get_arr()+i) = param[i];
            }
            return;
        }
                
                
                     
         

        bool permutation(const array_util& param)  const
        {
            
           
            if(this->length() != param.length() ) return false;

            if(this->is_vec())
            {
                std::vector<elem> dcpy(this->get_vec()); //deep copy for const correctness
                std::sort(dcpy.begin(), dcpy.end());
                array_util<elem> cp(dcpy);

                do{
                    if(cp.equals(param)) return true; 
                }while(std::next_permutation(cp.get_vec().begin(), cp.get_vec().end()));
            
                return false;
            }
            //same method for arrays:

            elem dcpy[this->length()]; //deep copy for cpnst corretness
            for(int i=0;i<this->length();++i)
            {
                dcpy[i] = (*this)[i];
            }
            array_util<elem> cp(dcpy, this->length()); 
            std::sort(cp.get_arr(), cp.get_arr()+cp.length()); 
            
            do{
                if(cp.equals(param)) return true; 
            }while(std::next_permutation(cp.get_arr(), (cp.get_arr()+cp.length())));

            return false;    

        }

        //operator overloads

        bool operator==(const array_util &rhs)  const
        {
            return this->equals(rhs);
        }

        void operator=(const array_util& rhs) 
        {
            return this->copy(rhs);
        }

        elem operator[](int index) const
        {
            if(index > this->length()-1)
            {
                std::cout<< "Array index out of bond!"<<std::endl;
                exit(0);
            }
            if(this->is_vec())
            {
                return *(this->get_vec().begin()+index);
            }
            return *(this->get_arr()+index);
        }

        
        

    private:
        bool _is_vec = false;
        elem* _arr;
        std::vector<elem> _vec;
        std::vector<elem>* _original_vec;
        int _length;
        
};

#endif 